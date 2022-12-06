import {Layout, Menu, MenuProps} from "antd";
import "./Admin.css";
import React, {useState} from "react";
import {Route, Routes, useNavigate} from "react-router-dom";
import {Footer} from "../../components/Footer";
import {ProfileOutlined, RetweetOutlined, ShoppingCartOutlined} from "@ant-design/icons";
import {Presentations} from "./presentations/Presentations";
import {Process} from "./process/Process";
import {Logs} from "./logs/Logs";

const items = [
    {label: "工艺计算书", key: "process", icon: <RetweetOutlined/>},
    {label: "PPT超市", key: "presentations", icon: <ShoppingCartOutlined/>},
    {label: "日志", key: "logs", icon: <ProfileOutlined/>}
];

export const Admin: React.FC = () => {
    const [collapsed, setCollapsed] = useState(false);

    const navigate = useNavigate();
    const onClick: MenuProps['onClick'] = (e) => {
        navigate(e.key);
    };

    return (
        <Layout style={{minHeight: '100vh'}}>
            <Layout.Sider collapsible collapsed={collapsed} onCollapse={(value) => setCollapsed(value)}>
                <div className="logo"/>
                <Menu
                    theme="dark"
                    onClick={onClick}
                    mode="inline"
                    items={items}
                />
            </Layout.Sider>
            <Layout>
                <Layout.Content>
                    <Routes>
                        <Route path={"process"} element={<Process/>}/>
                        <Route path={"presentations"} element={<Presentations/>}/>
                        <Route path={"logs"} element={<Logs/>}/>
                    </Routes>
                </Layout.Content>
                <Footer/>
            </Layout>
        </Layout>
    );
};



