import { Layout } from "antd";
import { Content, Footer, Header } from "antd/lib/layout/layout";
import { BrowserRouter, Routes, Route } from "react-router-dom";
import Admin from "./Pages/Admin/Admin";
import Home from "./Pages/Home/Home";
import "./App.css";

export default function App() {
  return (
    <Layout className="layout">
      <Header className="site-layout-header"></Header>
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
        ©2022 Coded by JetSnail
      </Footer>
    </Layout>
  );
}
