import {Button, Divider, Form, Input, Modal, theme, Tree, TreeSelect, Typography} from "antd";
import React, {useContext, useEffect, useState} from "react";
import {TopicDto, TreeSelectViewModel, TreeViewModel} from "dto";
import {PlusOutlined} from "@ant-design/icons";
import axios from "axios";
import {ActionCard} from "../../../components/ActionCard";
import {PresentationContext} from "./Presentations";
import produce from "immer";

const {useToken} = theme;


const update = (list: TreeViewModel<TopicDto>[], key: React.Key, children: TreeViewModel<TopicDto>[]): TreeViewModel<TopicDto>[] =>
    list.map((node) => {
        if (node.key === key) {
            return {
                ...node,
                children,
            };
        }
        if (node.children) {
            return {
                ...node,
                children: update(node.children, key, children),
            };
        }
        return node;
    });

const append = (list: TreeViewModel<TopicDto>[], newNode: TreeViewModel<TopicDto>): TreeViewModel<TopicDto>[] =>
    list.map((node) => {
        if (node.key === newNode.parentId) {
            return {
                ...node,
                children: produce(node.children, draft => {
                    draft.push(newNode)
                }),
            };
        }
        if (node.children) {
            return {
                ...node,
                children: append(node.children, newNode),
            };
        }
        return node;
    });

export const Topics = () => {
    const {token} = useToken();
    const {topics, setTopics, current, setCurrent} = useContext(PresentationContext);
    const [isModalOpen, setIsModalOpen] = useState(false);

    const onLoadData = async (treeNode: TreeViewModel<TopicDto>) => {
        const res = await axios.create()
            .get(`http://localhost:5000/topics/${treeNode.id}/topics`);
        const subTopics: TreeViewModel<TopicDto>[] = res.data.map((item: TopicDto) => {
            return {
                ...item,
                key: item.id,
                title: item.name,
                isLeaf: item.topics.length === 0
            };
        })

        setTopics(update(topics, treeNode.key, subTopics));
    };

    const handleSelected = (selectedKeys: (string | number)[], info: any) => {
        const selected = info.node as TreeViewModel<TopicDto>;
        setCurrent(selected);
    };

    useEffect(() => {
        const load = async () => {
            const res = await axios.create().get(`http://localhost:5000/topics/root`);
            setTopics(
                res.data.map((item: TopicDto) => {
                    return {
                        ...item,
                        key: item.id,
                        title: item.name,
                    };
                })
            );
        };

        load();
    }, [])

    return (
        <ActionCard title={"主题"}>
            {/* 极速 */}
            <Divider orientation={"left"} plain>极速</Divider>
            <Tree loadData={onLoadData} treeData={topics.filter(x => x.isFast)} onSelect={handleSelected}/>

            {/* 标准 */}
            <Divider orientation={"left"} plain>标准</Divider>
            <Tree loadData={onLoadData} treeData={topics.filter(x => !x.isFast)} onSelect={handleSelected}/>

            {/* 新建 */}
            <Divider/>
            <div style={{
                display: "flex",
                alignItems: "flex-start",
                background: token.colorPrimary,
                borderRadius: token.borderRadius,
                color: token.colorTextLightSolid,
                cursor: "pointer",
            }}
                 onClick={() => setIsModalOpen(true)}
            >
                    <span style={{
                        alignSelf: "stretch",
                        width: "24px",
                        lineHeight: "24px",
                        textAlign: "center",
                    }}>
                        <PlusOutlined/>
                    </span>
                <span style={{padding: "0 4px", minHeight: "24px", lineHeight: "24px"}}>
                    <span>新建主题</span>
                </span>
            </div>

            <Modal open={isModalOpen} onCancel={() => setIsModalOpen(false)} footer={null}
                   title={<Typography.Title level={3}>新建主题</Typography.Title>}>
                <NewTopicForm onFinish={(newTopic: TopicDto) => {
                    setIsModalOpen(false);
                    const updated = append(topics, {
                            children: [],
                            isLeaf: false,
                            key: newTopic.id,
                            title: newTopic.name, ...newTopic
                        }
                    )
                    setTopics(updated);
                }}/>
            </Modal>
        </ActionCard>
    )
}

const NewTopicForm = ({onFinish}: { onFinish: (item: TopicDto) => void }) => {
    const [data, setData] = useState<TreeSelectViewModel<TopicDto>[]>([]);
    const [parentId, setParentId] = useState(0);
    const [name, setName] = useState<string>("");

    const loadAsync = async ({id}: any) => {
        const res = await axios.create().get(`http://localhost:5000/topics/${id}/topics`);
        if (res.status === 200) {
            const subTopics = res.data.map((item: TopicDto) => {
                return {
                    ...item,
                    key: item.id,
                    title: item.name,
                    isLeaf: item.topics.length == 0,
                    value: item.id,
                    pId: item.parentId
                };
            })
            setData(data.concat(subTopics));
        }
    }

    useEffect(() => {
        const load = async () => {
            const res = await axios.create().get(`http://localhost:5000/topics/root`);
            setData(
                res.data.map((item: TopicDto) => {
                    return {
                        ...item,
                        key: item.id,
                        value: item.id,
                        title: item.name,
                        isLeaf: item.topics.length == 0
                    };
                })
            );
        }

        load();
    }, [])

    const handleFinish = async () => {
        const res = await axios.create().post(`http://localhost:5000/topics`, {name, parentId});
        if (res.status == 200) {
            // reload if success
            const newTopic = {
                ...res.data,
                key: res.data.id,
                value: res.data.id,
                title: res.data.name,
                isLeaf: true
            };
            setData(data.concat(newTopic));
            onFinish(res.data as TopicDto);
        }
    }

    return (
        <>
            <Form onFinish={handleFinish}>
                <Form.Item label="父主题" name="parentName">
                    <TreeSelect
                        treeDataSimpleMode
                        style={{width: '100%'}}
                        value={parentId}
                        dropdownStyle={{maxHeight: 400, overflow: 'auto'}}
                        placeholder="请选择父主题"
                        onChange={(value) => {
                            setParentId(value);
                        }}
                        loadData={loadAsync}
                        treeData={data}
                    />
                </Form.Item>
                <Form.Item label="名称" name="name" rules={[{required: true, message: '请输入名称'}]}>
                    <Input value={name} onChange={(e) => setName(e.target.value)}/>
                </Form.Item>
                <Form.Item wrapperCol={{offset: 8, span: 16}}>
                    <Button type="primary" htmlType="submit">
                        Submit
                    </Button>
                </Form.Item>
            </Form>
        </>
    )
}