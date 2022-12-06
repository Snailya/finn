import {Col, Row} from "antd";
import {Blocks} from "../components/Blocks";
import {Formulas} from "../components/Formulas";
import React from "react";

export const Process = () => {
    return (
        <div style={{padding: "16px"}}>
            <Row className={"admin__block"} gutter={16}>
                <Col>
                    <Blocks/>
                </Col>
                <Col>
                    <Formulas/>
                </Col>
            </Row>
        </div>
    )
}