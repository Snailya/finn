import React, {ReactNode} from "react";
import {Link} from "react-router-dom";
import {Card, Col, Image, Layout, Row, theme, Typography} from "antd";

import {Footer} from "../../components/Footer";

const {useToken} = theme;

export const Mode = () => {
    return (
        <Layout style={{minHeight: "100vh"}}>
            <Layout.Content style={{height: "0"}}>
                <Row
                    style={{
                        height: "100%",
                        justifyContent: "space-evenly",
                        alignItems: "stretch",
                        padding: "32px"
                    }}
                >
                    <Col span={8}>
                        <ModeCard
                            label={"PPT超市-简易模式"}
                            route={"/slides/shop?fast=true"}
                            icon={<Image src="/fast-mode.gif" preview={false}/>}
                        />
                    </Col>
                    <Col span={8}>
                        <ModeCard
                            label={"PPT超市-完整模式"}
                            route={"/slides/shop"}
                            icon={<Image src="/standard-mode.gif" preview={false}/>}
                        />
                    </Col>
                </Row>
            </Layout.Content>
            <Footer/>
        </Layout>
    );
};

const ModeCard = ({
                      label,
                      route,
                      icon,
                  }: {
    label: string;
    route: string;
    icon: ReactNode;
}) => {
    const {token} = useToken();

    return (
        <Link to={route}>
            <Card hoverable style={{height: "100%"}}>
                <div
                    style={{
                        display: "flex",
                        flexDirection: "column",
                        placeItems: "center",
                    }}
                >
                    <div style={{fontSize: "48px"}}>{icon}</div>
                    <Typography.Title>{label}</Typography.Title>
                </div>
            </Card>
        </Link>
    );
};
