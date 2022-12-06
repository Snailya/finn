import React from "react";
import {BrowserRouter, Route, Routes} from "react-router-dom";

import {ConfigProvider} from "antd";
import 'antd/dist/reset.css';

import {Admin} from "./pages/admin/Admin";
import {Home} from "./pages/home/Home";
import {Index as SlideIndex} from "./pages/slides/Index";
import {SlidesShop} from "./pages/slides/SlidesShop";
import {NotFound} from "./pages/notFound/NotFound";
import {Mode} from "./pages/slides/Mode";

export default function App() {
    return (
        <ConfigProvider theme={{
            token: {}
        }}>
            <BrowserRouter>
                <Routes>
                    <Route path="/*">
                        <Route index path="process" element={<Home/>}/>
                        <Route path="slides">
                            <Route index element={<SlideIndex/>}/>
                            <Route path="mode" element={<Mode/>}/>
                            <Route path="shop" element={<SlidesShop/>}/>
                        </Route>
                    </Route>
                    <Route path="/admin/*" element={<Admin/>}/>
                    <Route path="*" element={<NotFound/>}/>
                </Routes>
            </BrowserRouter>
        </ConfigProvider>
    );
}

