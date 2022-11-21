import {Divider} from "antd";
import "./Admin.css";
import {Blocks} from "./components/Blocks";
import {Formulas} from "./components/Formulas";
import {Logs} from "./components/Logs";

export const Admin: React.FC = () => {
    return (
        <div className="admin">
            <Blocks/>
            <Divider/>
            <Formulas/>
            <Divider/>
            <Logs/>
        </div>
    );
};
