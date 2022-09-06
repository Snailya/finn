import "./Logs.css";

import {
  CheckCircleOutlined,
  CloseCircleOutlined,
  SyncOutlined,
} from "@ant-design/icons";
import { Table, Tag, Typography } from "antd";
import type { ColumnsType } from "antd/es/table";
import React, { useEffect, useState } from "react";
import { apiFetch } from "../../../service";
import { red, blue, green } from "@ant-design/colors";

const { Title } = Typography;

interface RequestLog {
  id: number;
  type: string;
  status: string;
  input: string;
  output: string;
  created: string;
}

const columns: ColumnsType<RequestLog> = [
  {
    title: "时间",
    dataIndex: "created",
    key: "created",
  },
  {
    title: "类型",
    dataIndex: "type",
    key: "type",
    render: (value: string, record: RequestLog) => (
      <Tag color={value === "layout" ? "geekblue" : "green"}>
        {value.toUpperCase()}
      </Tag>
    ),
  },
  {
    title: "状态",
    dataIndex: "status",
    key: "status",
    render: (value: string, record: RequestLog) => {
      switch (value) {
        case "pending":
          return <SyncOutlined spin style={{ color: blue[5] }} />;
        case "done":
          return <CheckCircleOutlined style={{ color: green[5] }} />;
        case "error":
          return <CloseCircleOutlined style={{ color: red[4] }} />;
        default:
          break;
      }
    },
    filters: [
      {
        text: "挂起",
        value: "pending",
      },
      {
        text: "完成",
        value: "done",
      },
      {
        text: "错误",
        value: "error",
      },
    ],
    onFilter: (value: any, record) => record.status.indexOf(value) === 0,
  },
  {
    title: "输入",
    dataIndex: "input",
    key: "input",
  },
  {
    title: "输出",
    dataIndex: "output",
    key: "output",
  },
];

export const Logs: React.FC = () => {
  const [data, setData] = useState<RequestLog[]>([]);

  useEffect(() => {
    apiFetch("/logs")
      .then((res) => res.json())
      .then((body) => {
        if (body.code === 0) {
          setData(body.data);
        }
      });
  }, []);

  return (
    <Table
      className="logs__log-table"
      title={() => <Title level={3}>请求记录</Title>}
      rowKey={(record) => record.id}
      columns={columns}
      dataSource={data}
      pagination={{ simple: true }}
    />
  );
};
