import {Col, Row} from "antd";
import React from "react";
import {Formulas} from "./Formulas";
import {Blocks} from "./Blocks";

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