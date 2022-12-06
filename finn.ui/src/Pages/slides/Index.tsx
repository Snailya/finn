import React from "react";
import {Image} from "antd";
import {useNavigate} from "react-router-dom";

export const Index = () => {
    const navigate = useNavigate();
    return (
        <div style={{textAlign: "center"}}>
            <Image
                sizes="50%"
                preview={false}
                src="/bg1664x943.png"
                alt="bg1664x943"
                onClick={() => navigate("/slides/mode")}
            />
            ;
        </div>
    );
};
