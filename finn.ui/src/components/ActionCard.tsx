import {Col, Row, theme, Typography} from "antd";
import React, {ReactNode} from "react";

const {useToken} = theme;

export interface ActionCardProps {
    title: string;
    children: ReactNode;
    actions?: ReactNode | undefined;
}

export const ActionCard = ({title, children, actions}: ActionCardProps) => {
    const {token} = useToken();
    return (
        <div style={{background: token.colorBgBase, padding: "16px", borderRadius: token.borderRadius}}>
            <Row>
                <Col flex={1}>
                    <Typography.Title level={3}>{title}</Typography.Title>
                </Col>
                <Col>
                    {actions}
                </Col>
            </Row>
            {children}
        </div>
    )
}