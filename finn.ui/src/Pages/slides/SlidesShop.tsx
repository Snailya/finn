import React, {useEffect, useState} from "react";
import {useSearchParams} from "react-router-dom";
import {
    Affix,
    Button,
    Checkbox,
    Col,
    Drawer,
    Empty,
    FloatButton,
    Image,
    Layout,
    List,
    Menu,
    Row,
    Skeleton,
    Space,
    theme,
    Typography
} from "antd";
import {CloseOutlined, ShoppingCartOutlined} from "@ant-design/icons";
import axios from "axios";

import {Footer} from "../../components/Footer";

const {useToken} = theme;

interface Slide {
    id: number;
    image: string
}

const CartContext = React.createContext({
    slides: [] as Slide[],
    onAdd: (item: Slide) => {
    },
    onRemove: (id: number) => {
    }
});

export const SlidesShop = () => {
    const [selectedTopic, setSelectedTopic] = useState<number[]>([]);
    const [selectedSlides, setSelectedSlides] = useState<Slide[]>([])
    const [open, setOpen] = useState(false);

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

        <Layout style={{minHeight: '100vh'}}>
            <Layout.Sider theme={"light"} style={{
                minWidth: "200px"
            }}>
                <TopicMenu onSelect={setSelectedTopic}/>
            </Layout.Sider>
            <Layout>
                <Layout.Content>
                    {selectedTopic ? selectedTopic.map((topicId: number) => (
                        <SlidesGallery key={topicId} id={topicId}/>
                    )) : (<div>请选择主题</div>)}
                </Layout.Content>
                <Footer/>
            </Layout>

            <FloatButton style={{marginRight: "8px"}} icon={<ShoppingCartOutlined/>}
                         onClick={() => setOpen(true)}/>

            {/* 购物车 */}
            <CartContext.Provider value={{
                slides: selectedSlides,
                onAdd: handleAdd,
                onRemove: handleRemove
            }}>
                <Drawer title="购物车" placement="right" onClose={() => setOpen(false)} open={open}>
                    <ShoppingCart items={selectedSlides}/>
                </Drawer>
            </CartContext.Provider>
        </Layout>
    );
};

const TopicMenu = ({onSelect}: { onSelect: (item: any) => void }) => {
    const [searchParams] = useSearchParams({fast: "false"});
    const fast = searchParams.get("fast") as unknown as boolean;

    const [topics, setTopics] = useState([]);


    const handleTopicSelected = ({item, key, keyPath, selectedKeys, domEvent}: any) => {
        onSelect(item.props.topics);
    }

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

    return (
        <div>
            <Row align={"middle"}>
                <Col flex={"1"}>
                    <Typography.Title style={{marginTop: "12px", paddingLeft: "28px"}}
                                      level={3}>主题</Typography.Title>
                </Col>
            </Row>
            <Menu
                mode="inline"
                items={topics}
                onSelect={handleTopicSelected}
            />
        </div>
    )
}


const ShoppingCart = ({items}: { items: Slide[] }) => {
    return (
        <div style={{height: "100%", display: "flex", flexDirection: "column"}}>
            <div style={{flexGrow: "1"}}>
                {items.length < 0 ?
                    <Empty/> :
                    <CartContext.Consumer>
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
                    </CartContext.Consumer>

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

                <Button type={"primary"} disabled={true}
                        href={`http://localhost:5000/topics/${id}/slides/download`}>下载</Button>
            </div>
            <div style={{display: "flex", gap: "8px", flexFlow: "wrap", justifyContent: "center"}}>
                {slides.length > 0 ? slides.map((slideId: number) => (
                    <GalleryItem key={slideId} id={slideId}/>
                )) : (
                    <Empty description={false} image={Empty.PRESENTED_IMAGE_SIMPLE}/>
                )}
            </div>
        </div>
    )
}

export const GalleryItem = ({id}: { id: number }) => {
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
        <CartContext.Consumer>
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
        </CartContext.Consumer>
    )
}




