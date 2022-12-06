import "./Formulas.css";

import {Col, Descriptions, Form, message, Row, Space, Table, Typography,} from "antd";
import {useEffect, useState} from "react";
import {apiFetch} from "../../../service";
import {FormulaDto, TableViewModel} from "dto";
import {EditableCell} from "../../../components/EditableCell";

const {Title} = Typography;

const TableTitle = () => (
    <Row align="middle" gutter={16}>
        <Col flex="auto">
            <Title level={3}>计算公式</Title>
        </Col>
        <Col>
            <div className="hint">
                查看
                <Typography.Link
                    href="https://github.com/sys27/xFunc/wiki/Supported-functions-and-operations"
                    target="_blank"
                >
                    支持的公式及运算符
                </Typography.Link>
            </div>
        </Col>
    </Row>
);

const ParameterDescription = () => (
    <Descriptions
        className="formulas__formula-description"
        title={<Title level={3}>参数列表</Title>}
        column={1}
        bordered
    >
        <Descriptions.Item label="xp">x轴方向上与原点的距离(mm)</Descriptions.Item>
        <Descriptions.Item label="yp">y轴方向上与原点的距离(mm)</Descriptions.Item>
        <Descriptions.Item label="zp">z轴方向上与原点的距离(mm)</Descriptions.Item>
        <Descriptions.Item label="xl">x轴方向上的长度(mm)</Descriptions.Item>
        <Descriptions.Item label="yl">y轴方向上的长度(mm)</Descriptions.Item>
        <Descriptions.Item label="zl">z轴方向上的长度(mm)</Descriptions.Item>
    </Descriptions>
);

export const Formulas = () => {
    const [form] = Form.useForm();
    const [data, setData] = useState<TableViewModel<FormulaDto>[]>();
    const [editingKey, setEditingKey] = useState<string | number>(-1);

    useEffect(() => {
        apiFetch("/formulas")
            .then((res) => res.json())
            .then((body) => {
                if (body.code === 0) {
                    var data = body.data.map((item: FormulaDto) => {
                        return {
                            key: item.id,
                            ...item,
                        };
                    });
                    setData(data);
                }
            });
    }, []);

    const editRecord = (record: Partial<FormulaDto> & { key: React.Key }) => {
        form.setFieldsValue({
            id: record.id,
            type: record.type,
            expression: record.expression,
        });
        setEditingKey(record.key);
    };

    const cancel = () => {
        setEditingKey(-1);
    };

    const isEditing = (record: TableViewModel<FormulaDto>) =>
        record.key === editingKey;

    const saveRecord = async (record: TableViewModel<FormulaDto>) => {
        try {
            var newRecord = form.getFieldsValue(true);

            // call api to persist
            var formData = new FormData();
            formData.append("Id", newRecord.id.toString());
            formData.append("Type", newRecord.type);
            formData.append("Expression", newRecord.expression);

            apiFetch("/formulas", {
                method: "PUT",
                body: formData,
            })
                .then((res) => res.json())
                .then((body) => {
                    if (body.code === 0) {
                        message.success(`修改成功。`);

                        const newData = [...data!];
                        const index = newData.findIndex((item) => record.key === item.key);

                        if (index > -1) {
                            const item = newData[index];
                            newData.splice(index, 1, {...item, ...newRecord});
                            setData(newData);
                            setEditingKey(-1);
                        } else {
                            newData.push(record);
                            setData(newData);
                            setEditingKey(-1);
                        }
                    } else {
                        message.error(`修改失败：${body.msg}`);
                    }
                });
        } catch (errInfo) {
            console.log("数据校验失败：", errInfo);
        }
    };

    const deleteRecord = (record: TableViewModel<FormulaDto>) => {
        try {
            apiFetch(`/formulas/${record.key}`, {
                method: "DELETE",
            })
                .then((res) => res.json())
                .then((body) => {
                    if (body.code === 0) {
                        message.success(`删除成功`);

                        const newData = [...data!];
                        const index = newData.findIndex((item) => record.key === item.key);

                        if (index > -1) {
                            newData.splice(index, 1);
                            setData(newData);
                            setEditingKey(-1);
                        } else {
                            message.error(`删除失败：${body.msg}`);
                        }
                    }
                });
        } catch (errInfo) {
            console.log("删除失败：", errInfo);
        }
    };

    const columns = [
        {
            title: "类型",
            dataIndex: "type",
            width: "20%",
        },
        {
            title: "表达式",
            dataIndex: "expression",
            width: "60%",
            editable: true,
        },
        {
            title: "动作",
            dataIndex: "operation",
            render: (_: any, record: TableViewModel<FormulaDto>) => {
                const editable = isEditing(record);
                return editable ? (
                    <Space>
                        <Typography.Link onClick={() => saveRecord(record)}>
                            保存
                        </Typography.Link>
                        <Typography.Link onClick={cancel}>取消</Typography.Link>
                    </Space>
                ) : (
                    <Space>
                        <Typography.Link
                            disabled={editingKey !== -1}
                            onClick={() => editRecord(record)}
                        >
                            编辑
                        </Typography.Link>
                        <Typography.Link type="danger" onClick={() => deleteRecord(record)}>
                            删除
                        </Typography.Link>
                    </Space>
                );
            },
        },
    ];

    const mergedColumns = columns.map((col) => {
        if (!col.editable) {
            return col;
        }

        return {
            ...col,
            onCell: (record: TableViewModel<FormulaDto>) => ({
                record,
                dataIndex: col.dataIndex,
                title: col.title,
                editing: isEditing(record),
            }),
        };
    });

    return (
        <div className="formulas">
            <Form form={form} component={false}>
                <Table
                    className="formulas__formula-table"
                    title={TableTitle}
                    components={{
                        body: {
                            cell: EditableCell,
                        },
                    }}
                    dataSource={data}
                    columns={mergedColumns}
                    rowClassName="editable-row"
                    pagination={{
                        onChange: cancel,
                    }}
                />
            </Form>
            <ParameterDescription/>
        </div>
    );
};
