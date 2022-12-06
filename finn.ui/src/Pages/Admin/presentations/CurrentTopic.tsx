import React, {useContext} from "react";
import {Button, Descriptions, Empty, Space, Upload} from "antd";

import {SlideDto} from "dto";
import {ActionCard} from "../../../components/ActionCard";
import axios from "axios";
import {GalleryItem} from "../../slides/SlidesShop";
import produce from "immer";
import {PresentationContext} from "./Presentations";

const remove = (list: any[], key: React.Key) => {
    return list.map((item: any) => {
        return {...item}
    }).filter(item => {
        if ('children' in item) {
            item.children = remove(item.children, key);
        }
        return item.key !== key;
    });
}

export const CurrentTopic = () => {
    const {topics, setTopics, current, setCurrent} = useContext(PresentationContext);

    const handleDeleteTopic = async () => {
        if (!current) return;
        const res = await axios.create().delete(`http://localhost:5000/topics/${current.id}`);
        if (res.status === 204) {
            setCurrent(undefined);

            // update Topics Context
            const removed = remove(topics, current.key)
            setTopics(removed);
        }
    }

    const handleDeleteSlides = async () => {
        if (!current) return;
        const res = await axios.create().delete(`http://localhost:5000/topics/${current.id}/slides`);
        if (res.status === 204) {
            setCurrent(produce(current, draft => {
                draft.slides = []
            }))
        }
    }

    return (
        <div style={{display: "flex", flexDirection: "column", gap: "16px"}}>
            {
                current ?
                    <>
                        <ActionCard title={current.name} actions={current ? <Space>
                            <Button type={"primary"}>重命名</Button>
                            <Button type={"primary"} danger onClick={handleDeleteTopic}>删除</Button>
                        </Space> : null}>
                            <Descriptions>
                                <Descriptions.Item label="Id"><Space>
                                    {current.id}
                                </Space>
                                </Descriptions.Item>
                                <Descriptions.Item label="子主题">{current.topics.length}</Descriptions.Item>
                                <Descriptions.Item label="幻灯片">
                                    {current.slides.length}
                                </Descriptions.Item>
                            </Descriptions>
                        </ActionCard>

                        <ActionCard title={`${current.name}: 幻灯片`} actions={<Space>
                            <Upload accept={".pptx"}
                                    action={`http://localhost:5000/topics/${current.id}/slides`}
                                    showUploadList={false}
                                    onChange={({file}) => {
                                        if (file.status === "done") {
                                            setCurrent(produce(current, draft => {
                                                draft.slides.push(...file.response.map((x: SlideDto) => x.id))
                                            }))
                                        }
                                    }}
                            >
                                <Button type={"primary"}>上传</Button>
                            </Upload>
                            <Button type={"primary"} danger disabled={current.slides.length <= 0}
                                    onClick={handleDeleteSlides}>清空</Button>
                        </Space>}>
                            {current.slides.length > 0 ?
                                <div style={{
                                    display: "flex",
                                    gap: "8px",
                                    flexFlow: "wrap",
                                    justifyContent: "center"
                                }}>
                                    {current.slides.map((item: number) => <GalleryItem key={item} id={item}/>)}
                                </div> : <Empty/>}
                        </ActionCard>
                    </> : null
            }
        </div>
    )
}
