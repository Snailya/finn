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
import { RequestLogDto, TableRecord } from "dto";
import moment from "moment";

const { Title } = Typography;

const columns: ColumnsType<TableRecord<RequestLogDto>> = [
  {
    title: "时间",
    dataIndex: "created",
    key: "created",
    render: (value: string, record: TableRecord<RequestLogDto>) => (
      <>{moment(value).format("yyyy-MM-DD HH:mm:ss")}</>
    ),
    sorter: (a: TableRecord<RequestLogDto>, b: TableRecord<RequestLogDto>) => {
      return moment(a.created).valueOf() - moment(b.created).valueOf();
    },
    defaultSortOrder: "descend",
  },
  {
    title: "类型",
    dataIndex: "type",
    key: "type",
    render: (value: string, record: TableRecord<RequestLogDto>) => (
      <Tag color={value === "layout" ? "geekblue" : "green"}>
        {value.toUpperCase()}
      </Tag>
    ),
  },
  {
    title: "状态",
    dataIndex: "status",
    key: "status",
    render: (value: string, record: TableRecord<RequestLogDto>) => {
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
    onFilter: (value: any, record: TableRecord<RequestLogDto>) =>
      record.status.indexOf(value) === 0,
  },
  {
    title: "源文件",
    dataIndex: "origin",
    key: "origin",
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
  const [data, setData] = useState<TableRecord<RequestLogDto>[]>([]);

  useEffect(() => {
    apiFetch("/logs")
      .then((res) => res.json())
      .then((body) => {
        if (body.code === 0) {
          setData(
            body.data.map((item: RequestLogDto) => {
              return {
                key: item.id,
                ...item,
              };
            })
          );
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
