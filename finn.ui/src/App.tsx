import { Layout } from "antd";
import { Content, Footer } from "antd/lib/layout/layout";
import React from "react";
import { BrowserRouter, Routes, Route, Link } from "react-router-dom";
import Admin from "./Pages/Admin";
import Home from "./Pages/Home";

export default function App() {
  return (
    <Layout className="layout">
      <Content style={{ padding: "0 50px" }}>
        <div className="site-layout-content">
          <BrowserRouter>
            <Routes>
              <Route path="/admin" element={<Admin></Admin>} />
              <Route path="/" element={<Home></Home>} />
            </Routes>
          </BrowserRouter>
        </div>
      </Content>
      <Footer style={{ textAlign: "center" }}>
        Ant Design ©2018 Created by Ant UED
      </Footer>
    </Layout>
  );
}
