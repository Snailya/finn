import './CostTable.css';

import {Table, Typography} from "antd";
import type {ColumnsType} from "antd/es/table";
import {CostDto, TableRecord} from "dto";
import React from "react";
import {TableProps} from "../../../components/TableProps";

const {Text, Title} = Typography;

const typeName: { [id: string]: string } = {
    platform: "平台",
};

const columns: ColumnsType<TableRecord<CostDto>> = [
    {
        title: "类型",
        dataIndex: "category",
        render: (value: string, record: TableRecord<CostDto>) => (
            <>{typeName[value]}</>
        ),
    },
    {
        title: "价格(万元)",
        dataIndex: "value",
        render: (value: number, record: TableRecord<CostDto>) => (
            <>{value.toFixed(0)}</>
        ),
    },
];

export const CostTable: React.FC<TableProps<CostDto>> = (
    props: TableProps<CostDto>
) => (
    <Table
        className="costs__cost-table"
        title={() => <Title level={4}>投资估算</Title>}
        size="small"
        columns={columns}
        dataSource={props.data}
        pagination={false}
        summary={(pageData) => {
            let total = 0;
            pageData.forEach(({value}) => {
                total += value;
            });

            return (
                <>
                    <Table.Summary.Row className='cost__table-summary'>
                        <Table.Summary.Cell index={0}>总价</Table.Summary.Cell>
                        <Table.Summary.Cell index={1}>
                            <Text>{total.toFixed(0)}</Text>
                        </Table.Summary.Cell>
                    </Table.Summary.Row>
                </>
            );
        }}
    />
);
