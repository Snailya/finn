import "./Blocks.css";

import {UploadOutlined} from "@ant-design/icons";
import {Button, message, Space, Table, Typography, Upload} from "antd";
import type {UploadChangeParam, UploadFile} from "antd/es/upload";
import type {ColumnsType} from "antd/es/table";

import React, {useCallback, useEffect, useState} from "react";

import {apiFetch} from "../../../service";
import {Viewer} from "./Viewer";
import {BlockDefinitionDto, TableViewModel} from "dto";

const {Title} = Typography;

export const Blocks: React.FC = () => {
    const [data, setData] = useState<TableViewModel<BlockDefinitionDto>[]>([]);
    const [previewUrl, setPreviewUrl] = useState<string>();

    const load = useCallback(() => {
        apiFetch(`/blocks`)
            .then((res) => res.json())
            .then((body) => {
                if (body.code === 0) {
                    setData(body.data);
                }
            });
    }, []);

    useEffect(() => {
        load();
    }, [load]);

    const onUploadChange = (info: UploadChangeParam<UploadFile>) => {
        if (info.file.status === "done") {
            if (info.file.response.code === 0) {
                message.success(`${info.file.name}上传成功`);
                load();
            } else
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
        (block: TableViewModel<BlockDefinitionDto>) => {
            apiFetch(`/blocks/${block.id}`, {method: "DELETE"})
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

    const columns: ColumnsType<TableViewModel<BlockDefinitionDto>> = [
        {
            title: "名称",
            dataIndex: "name",
        },
        {
            title: "动作",
            render: (_, record) => (
                <Space size="middle">
                    <Typography.Link onClick={() => onPreviewClick(record.id)}>
                        预览
                    </Typography.Link>
                    <Typography.Link type="danger" onClick={() => onDeleteClick(record)}>
                        删除
                    </Typography.Link>
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
                            <Button type="primary" icon={<UploadOutlined/>}>
                                上传
                            </Button>
                        </Upload>
                    </div>
                )}
                columns={columns}
                dataSource={data}
                pagination={{simple: true}}
            />
            <div className="blocks__block-viewer">
                <Viewer url={previewUrl}></Viewer>
            </div>
        </div>
    );
};
