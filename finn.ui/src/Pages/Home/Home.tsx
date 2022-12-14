import "./Home.css";

import {message, Upload} from "antd";
import {DeleteOutlined, DownloadOutlined, InboxOutlined,} from "@ant-design/icons";

import {useCallback, useState} from "react";

import {saveAs} from "../../service";
import {CostTable} from "./compoents/CostTable";
import {CostDto, TableRecord} from "dto";
import type {RcFile, UploadChangeParam, UploadFile} from 'antd/es/upload/interface';

const {Dragger} = Upload;

export const Home: React.FC = () => {
    const [cost, setCost] = useState<TableRecord<CostDto>[]>();
    const [isDownloadIconVisible, setIsDownloadIconVisible] =
        useState<boolean>(false);

    const beforeUpload = useCallback((file: RcFile) => {
        if (
            file.type ===
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" ||
            file.name.endsWith(".dxf")
        ) {
            return true;
        }
        return false;
    }, []);

    const onChange = useCallback((info: UploadChangeParam<UploadFile>) => {
        const {status} = info.file;

        if (status === "uploading") {
            return;
        }

        if (status === "removed") {
            setCost(undefined);
            return;
        }

        if (status === "error") {
            message.error(`${info.file.name}上传失败。`);
            return;
        }

        if (info.file.response.code !== 0) {
            message.error(`${info.file.name}上传失败：${info.file.response.msg}。`);
            return;
        }
        message.success(`${info.file.name}上传成功。`);
        if (info.file.name?.endsWith(".xlsx")) {
            info.file.url = `${process.env.REACT_APP_BACKEND_URL}/files/${info.file.response.data}`;
            setIsDownloadIconVisible(true);

            saveAs(info.file.url, info.file.fileName ?? "");
        } else if (info.file.name?.endsWith(".dxf")) {
            setCost(
                info.file.response.data.map(
                    (x: { category: string; value: number }) => {
                        return {key: x.category, ...x};
                    }
                )
            );
            setIsDownloadIconVisible(false);
        }
    }, []);

    return (
        <div className="home">
            <div className="home__result">{cost && <CostTable data={cost}/>}</div>
            <div className="home__uploader">
                <Dragger
                    className="upload"
                    name="file"
                    accept=".xlsx,.dxf"
                    maxCount={1}
                    multiple={false}
                    action={`${process.env.REACT_APP_BACKEND_URL}/files/upload`}
                    onDrop={() => {
                        setCost(undefined);
                    }}
                    beforeUpload={beforeUpload}
                    onChange={onChange}
                    showUploadList={{
                        showDownloadIcon: isDownloadIconVisible,
                        downloadIcon: <DownloadOutlined/>,
                        showRemoveIcon: true,
                        removeIcon: <DeleteOutlined/>,
                    }}
                >
                    <p className="ant-upload-drag-icon">
                        <InboxOutlined/>
                    </p>
                    <p className="ant-upload-text">单击或拖拽文件至此位置上传</p>
                    <p className="ant-upload-hint">计算表可生成图纸，图纸可生成价格。</p>
                </Dragger>
            </div>
        </div>
    );
};
