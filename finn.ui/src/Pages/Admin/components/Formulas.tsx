import "./Formulas.css";

import {
  Button,
  Col,
  Descriptions,
  Form,
  Input,
  InputNumber,
  message,
  Row,
  Space,
  Table,
  Typography,
} from "antd";
import React, { useEffect, useState } from "react";
import { apiFetch } from "../../../service";

interface Formula {
  key: number;
  type: string;
  expression: number;
}

interface EditableCellProps extends React.HTMLAttributes<HTMLElement> {
  editing: boolean;
  dataIndex: string;
  title: any;
  inputType: "number" | "text";
  record: Formula;
  index: number;
  children: React.ReactNode;
}

const EditableCell = ({
  editing,
  dataIndex,
  title,
  inputType,
  record,
  index,
  children,
  ...restProps
}: EditableCellProps) => {
  const inputNode = inputType === "number" ? <InputNumber /> : <Input />;
  return (
    <td {...restProps}>
      {editing ? (
        <Form.Item
          name={dataIndex}
          style={{
            margin: 0,
          }}
          rules={[
            {
              required: true,
              message: `Please Input ${title}!`,
            },
          ]}
        >
          {inputNode}
        </Form.Item>
      ) : (
        children
      )}
    </td>
  );
};

const { Title } = Typography;

export const Formulas = () => {
  const [form] = Form.useForm();
  const [data, setData] = useState<any[]>();
  const [editingKey, setEditingKey] = useState(-1);

  useEffect(() => {
    apiFetch("/formulas")
      .then((res) => res.json())
      .then((body) => {
        if (body.code === 0) {
          setData(
            body.data.map(
              (i: { id: number; type: string; expression: string }) => {
                return {
                  key: i.id,
                  type: i.type,
                  expression: i.expression,
                };
              }
            )
          );
        }
      });
  }, []);

  const isEditing = (record: Formula) => record.key === editingKey;

  const edit = (record: Formula) => {
    form.setFieldsValue({
      name: "",
      age: "",
      address: "",
      ...record,
    });
    setEditingKey(record.key);
  };

  const cancel = () => {
    setEditingKey(-1);
  };

  const save = async (key: number) => {
    try {
      const row = await form.validateFields();

      // call api to persist
      var formData = new FormData();
      formData.append("Id", key.toString());
      formData.append("Type", row.type);
      formData.append("Expression", row.expression);

      apiFetch("/formulas", {
        method: "PUT",
        body: formData,
      })
        .then((res) => res.json())
        .then((body) => {
          if (body.code === 0) {
            message.success(`修改成功。`);

            const newData = [...data!];
            const index = newData.findIndex((item) => key === item.key);

            if (index > -1) {
              const item = newData[index];
              newData.splice(index, 1, { ...item, ...row });
              setData(newData);
              setEditingKey(-1);
            } else {
              newData.push(row);
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

  const deleteRecord = (record: Formula) => {
    try {
      // call api to delete
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
      render: (_: any, record: Formula) => {
        const editable = isEditing(record);
        return editable ? (
          <Space>
            <Typography.Link onClick={() => save(record.key)}>
              保存
            </Typography.Link>
            <Typography.Link onClick={cancel}>取消</Typography.Link>
          </Space>
        ) : (
          <Space>
            <Typography.Link
              disabled={editingKey !== -1}
              onClick={() => edit(record)}
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
      onCell: (record: Formula) => ({
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
          title={() => (
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
          )}
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
      <Descriptions
        className="formulas__formula-description"
        title={<Title level={3}>参数列表</Title>}
        column={1}
        bordered
      >
        <Descriptions.Item label="xp">
          x轴方向上与原点的距离(mm)
        </Descriptions.Item>
        <Descriptions.Item label="yp">
          y轴方向上与原点的距离(mm)
        </Descriptions.Item>
        <Descriptions.Item label="zp">
          z轴方向上与原点的距离(mm)
        </Descriptions.Item>
        <Descriptions.Item label="xl">x轴方向上的长度(mm)</Descriptions.Item>
        <Descriptions.Item label="yl">y轴方向上的长度(mm)</Descriptions.Item>
        <Descriptions.Item label="zl">z轴方向上的长度(mm)</Descriptions.Item>
      </Descriptions>
    </div>
  );
};
