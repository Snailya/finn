import { Divider } from "antd";
import "./Admin.css";
import { Blocks } from "./components/Blocks";
import { Logs } from "./components/Logs";

const Admin: React.FC = () => {
  return (
    <div className="admin">
      <Blocks />
      <Divider />
      <Logs />
    </div>
  );
};

export default Admin;
