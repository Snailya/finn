import {Layout} from "antd";
import {Content, Footer, Header} from "antd/lib/layout/layout";
import {BrowserRouter, Routes, Route} from "react-router-dom";
import {Admin} from "./pages/admin/Admin";
import {Home} from "./pages/home/Home";
import "./App.css";
import React from "react";
import {Slide} from "./pages/slide/Slide";

export default function App() {
    return (
        <Layout className="layout">
            <Header className="site-layout-header"></Header>
            <Content style={{padding: "0 50px"}}>
                <div className="site-layout-content">
                    <BrowserRouter>
                        <Routes>
                            <Route path="/admin" element={<Admin></Admin>}/>
                            <Route path="/" element={<Home></Home>}/>
                            <Route path="/slide" element={<Slide></Slide>}/>
                        </Routes>
                    </BrowserRouter>
                </div>
            </Content>
            <Footer style={{textAlign: "center"}}>©2022 Coded by JetSnail</Footer>
        </Layout>
    );
}
