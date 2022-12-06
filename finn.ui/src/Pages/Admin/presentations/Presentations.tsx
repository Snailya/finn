import {Col, Row,} from "antd";
import React, {createContext, useState} from "react";
import {TopicDto, TreeViewModel} from "dto";
import {Topics} from "./Topics";
import {CurrentTopic} from "./CurrentTopic";

export const PresentationContext = createContext({
    topics: [] as TreeViewModel<TopicDto>[],
    setTopics: (value: TreeViewModel<TopicDto>[]) => {
    },
    current: undefined as TreeViewModel<TopicDto> | undefined,
    setCurrent: (value: TreeViewModel<TopicDto> | undefined) => {
    }
})

export const Presentations = () => {
    const [topics, setTopics] = useState<TreeViewModel<TopicDto>[]>([]);
    const [current, setCurrent] = useState<TreeViewModel<TopicDto> | undefined>(undefined);
    const value = {topics, setTopics, current, setCurrent};

    return (
        <PresentationContext.Provider value={value}>
            <div style={{padding: "16px"}}>
                <Row gutter={16}>
                    <Col>
                        <Topics/>
                    </Col>
                    <Col flex={1} style={{width: "0"}}>
                        <CurrentTopic/>
                    </Col>
                </Row>
            </div>
        </PresentationContext.Provider>
    )
}


