import {
    Affix,
    Button,
    Checkbox,
    Drawer,
    Empty,
    FloatButton,
    Image,
    Layout,
    List,
    Menu,
    Skeleton,
    Space,
    theme,
    Typography
} from "antd";
import React, {useEffect, useState} from "react";
import axios from "axios";
import {Footer} from "../../components/Footer";
import {CloseOutlined, ShoppingCartOutlined} from "@ant-design/icons";

const {useToken} = theme;

interface Slide {
    id: number;
    image: string
}

const SelectedSlidesContext = React.createContext({
    slides: [] as Slide[],
    onAdd: (item: Slide) => {
    },
    onRemove: (id: number) => {
    }
});

export const SlidesShop = ({fast}: { fast: boolean }) => {
    const {token, theme} = useToken();
    const [topics, setTopics] = useState([]);
    const [selectedTopic, setSelectedTopic] = useState<number[]>([]);
    const [selectedSlides, setSelectedSlides] = useState<Slide[]>([])
    const [open, setOpen] = useState(false);

    useEffect(() => {
        axios
            .create()
            .get(`http://localhost:5000/topics/root?fast=${fast}`)
            .then((res) => {
                setTopics(
                    res.data.map((item: any) => {
                        return {
                            ...item,
                            key: item.id,
                            label: item.name,
                        };
                    })
                );
            });
    }, [])

    const handleTopicSelected = ({item, key, keyPath, selectedKeys, domEvent}: any) => {
        setSelectedTopic(item.props.topics);
    }

    const handleAdd = (item: { id: number; image: string }) => {
        setSelectedSlides(x => [...x, item])
    }

    const handleRemove = (id: number) => {
        setSelectedSlides(x => {
            const index = x.findIndex((element) => element.id === id);
            const previous = [...x];
            previous.splice(index, 1);
            return previous;
        })
    }


    return (
        <SelectedSlidesContext.Provider value={{
            slides: selectedSlides,
            onAdd: handleAdd,
            onRemove: handleRemove
        }}>
            <Layout style={{minHeight: '100vh'}}>
                <Layout.Sider theme={"light"} style={{
                    minWidth: "200px"
                }}>
                    <div>
                        <Typography.Title style={{marginTop: "12px", paddingLeft: "28px"}}
                                          level={3}>主题</Typography.Title>
                        <Menu
                            mode="inline"
                            items={topics}
                            onSelect={handleTopicSelected}
                        />
                    </div>
                </Layout.Sider>
                <Layout>
                    <Layout.Content>
                        {selectedTopic ? selectedTopic.map((topicId: number) => (
                            <SlidesGallery key={topicId} id={topicId}/>
                        )) : (<div>请选择主题</div>)}
                    </Layout.Content>
                    <Footer/>
                </Layout>
                <FloatButton style={{marginRight: "8px"}} icon={<ShoppingCartOutlined/>} onClick={() => setOpen(true)}/>
                <Drawer title="购物车" placement="right" onClose={() => setOpen(false)} open={open}>
                    <ShoppingCart items={selectedSlides}/>
                </Drawer>
            </Layout>
        </SelectedSlidesContext.Provider>
    );
};

const ShoppingCart = ({items}: { items: Slide[] }) => {
    return (
        <div style={{height: "100%", display: "flex", flexDirection: "column"}}>
            <div style={{flexGrow: "1"}}>
                {items.length < 0 ?
                    <Empty/> :
                    <SelectedSlidesContext.Consumer>
                        {
                            ({slides, onAdd, onRemove}) =>
                                <List
                                    dataSource={items}
                                    renderItem={(item) => (
                                        <List.Item>
                                            <Space>
                                                <Button type={"link"} icon={<CloseOutlined/>}
                                                        onClick={() => onRemove(item.id)}></Button>
                                                <Image
                                                    style={{border: "1px solid"}} key={item.id} width={"150px"}
                                                    preview={false}
                                                    src={item.image}/>
                                            </Space>
                                        </List.Item>
                                    )}
                                />
                        }
                    </SelectedSlidesContext.Consumer>

                }
            </div>
            <Affix offsetBottom={16}>
                <div style={{display: "flex", justifyContent: "right"}}>
                    <Button
                        type={"primary"}
                        href={`http://localhost:5000/slides/download?${items
                            .map((item) => `ids=${item.id}`)
                            .join("&")}`}
                    >
                        生成
                    </Button>
                </div>
            </Affix>
        </div>
    )
}

export const SlidesGallery = ({id}: { id: number }) => {
    const [label, setLabel] = useState();
    const [slides, setSlides] = useState([]);

    useEffect(() => {
        axios
            .create()
            .get(`http://localhost:5000/topics/${id}`)
            .then((res) => {
                setLabel(res.data.name);
                setSlides(
                    res.data.slides
                );
            });
    }, [id])


    return (
        <div style={{background: "white", padding: "8px 16px", margin: "0 16px 16px 16px"}}>
            <div style={{display: "flex"}}>
                <Typography.Title level={4} style={{flexGrow: "1"}}>{label}</Typography.Title>
                <Button type={"primary"}
                        href={`http://localhost:5000/topics/${id}/slides/download`}>下载</Button>
            </div>
            <div style={{display: "flex", gap: "8px", flexFlow: "wrap"}}>
                {slides.length > 0 ? slides.map((slideId: number) => (
                    <GalleryItem key={slideId} id={slideId}/>
                )) : (
                    <Empty/>
                )}
            </div>
        </div>
    )
}

const GalleryItem = ({id}: { id: number }) => {
    const [isLoading, setIsLoading] = useState<boolean>(true);
    const [isSelected, setIsSelected] = useState<boolean>(false);
    const [src, setSrc] = useState("");

    useEffect(() => {
        axios
            .create()
            .get(`http://localhost:5000/slides/${id}`)
            .then((res) => {
                setSrc(res.data.image);
                setIsLoading(false);
            });
    }, [id])

    return (
        <SelectedSlidesContext.Consumer>
            {({slides, onAdd, onRemove}) =>
                <div style={{border: "1px solid", position: "relative"}}>
                    {isLoading
                        ? (<Skeleton.Image active/>)
                        : (
                            <div>
                                <Image width={"250px"} src={src}/>
                                <div
                                    style={{
                                        position: "absolute",
                                        top: 0,
                                        left: 0,
                                        width: "100%",
                                        height: "100%",
                                        display: isSelected ? "block" : "none",
                                        background: "rgb(0 0 0 / 50%)",
                                    }}
                                />
                                <Checkbox
                                    style={{position: "absolute", top: 0, left: 0}}
                                    checked={slides.findIndex(x => x.id === id) > -1}
                                    onChange={(e) => {
                                        if (e.target.checked) onAdd({id, image: src});
                                        else onRemove(id);
                                    }}
                                />
                            </div>
                        )
                    }
                </div>}
        </SelectedSlidesContext.Consumer>
    )
}




