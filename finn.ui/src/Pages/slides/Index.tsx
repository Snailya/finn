import React, {ReactNode} from "react";
import {Link} from "react-router-dom";
import {Card, Col, Layout, Row, theme, Typography} from "antd";
import {PieChartTwoTone, ThunderboltTwoTone} from "@ant-design/icons";

import {Footer} from "../../components/Footer";

const {useToken} = theme;

export const Index = () => {
    return (
        <Layout style={{minHeight: '100vh'}}>
            <Layout.Content style={{height: "0"}}>
                <Row style={{height: "100%", justifyContent: "space-evenly", alignItems: "center"}}>
                    <Col span={8}>
                        <ModeCard label={"PPT超市急速购物区"} route={"fast"} icon={<ThunderboltTwoTone/>}/>
                    </Col>
                    <Col span={8}>
                        <ModeCard label={"PPT超市"} route={"standard"} icon={<PieChartTwoTone/>}/>
                    </Col>
                </Row>
            </Layout.Content>
            <Footer/>
        </Layout>
    );
};

const ModeCard = ({label, route, icon}: { label: string, route: string, icon: ReactNode }) => {
    const {token} = useToken();

    return (
        <Link to={route}>
            <Card hoverable style={{height: "100%"}}>
                <div
                    style={{
                        display: "flex",
                        flexDirection: "column",
                        placeItems: "center",
                    }}>
                    <div style={{fontSize: "48px"}}>
                        {icon}
                    </div>
                    <Typography.Text>
                        {label}
                    </Typography.Text>
                </div>
            </Card>
        </Link>
    )
}
