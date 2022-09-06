import "./Blocks.css";

import { UploadOutlined } from "@ant-design/icons";
import { Button, message, Space, Table, Upload, Typography } from "antd";
import type { UploadChangeParam, UploadFile } from "antd/lib/upload";
import type { ColumnsType } from "antd/es/table";

import React, { useCallback, useEffect, useRef, useState } from "react";

import { apiFetch } from "../../../service";
import { Viewer } from "./Viewer";

interface BlockDefinition {
  id: number;
  name: string;
  filename: string;
}

const { Title } = Typography;

export const Blocks: React.FC = () => {
  const [data, setData] = useState<BlockDefinition[]>([]);
  const [previewUrl, setPreviewUrl] = useState<string>();

  useEffect(() => {
    apiFetch(`/blocks`)
      .then((res) => res.json())
      .then((body) => {
        if (body.code === 0) {
          setData(body.data);
        }
      });
  }, []);

  const onUploadChange = (info: UploadChangeParam<UploadFile>) => {
    if (info.file.status === "done") {
      if (info.file.response.code === 0)
        message.success(`${info.file.name}上传成功`);
      else
        message.error(
          `${info.file.name}上传失败: ${info.file.response.msg}`,
          10
        );
    } else if (info.file.status === "error") {
      message.error(`${info.file.name}上传失败`);
    }
  };

  const onPreviewClick = useCallback((id: number) => {
    setPreviewUrl(`${process.env.REACT_APP_BACKEND_URL}/blocks/${id}`);
  }, []);

  const onDeleteClick = useCallback(
    (block: BlockDefinition) => {
      apiFetch(`/blocks/${block.id}`, { method: "DELETE" })
        .then((res) => res.json())
        .then((body) => {
          if (body.code === 0) {
            message.success(`${block.name}删除成功。`);

            const index = data.indexOf(block);
            if (index !== -1) {
              data.splice(index, 1);
              setData([...data]);
            }
          } else {
            message.error(`${block.name}删除失败。`);
          }
        });
    },
    [data]
  );

  const columns: ColumnsType<BlockDefinition> = [
    {
      title: "名称",
      dataIndex: "name",
      key: "name",
    },
    {
      title: "动作",
      key: "actions",
      render: (_, record) => (
        <Space size="middle">
          <Button type="link" onClick={() => onPreviewClick(record.id)}>
            预览
          </Button>
          <Button type="link" danger onClick={() => onDeleteClick(record)}>
            删除
          </Button>
        </Space>
      ),
    },
  ];

  return (
    <div className="blocks__container">
      <Table
        className="blocks__block-table"
        rowKey={(record) => record.id}
        title={() => (
          <div className="blocks__block-table-header">
            <Title level={3} className="blocks__block-table-title">
              块
            </Title>
            <Upload
              name="file"
              accept=".dxf"
              showUploadList={false}
              action={`${process.env.REACT_APP_BACKEND_URL}/blocks/upload`}
              onChange={onUploadChange}
            >
              <Button type="primary" icon={<UploadOutlined />}>
                上传
              </Button>
            </Upload>
          </div>
        )}
        columns={columns}
        dataSource={data}
        pagination={{ simple: true }}
      />
      <div className="blocks__block-viewer">
        <Viewer url={previewUrl}></Viewer>
      </div>
    </div>
  );
};
