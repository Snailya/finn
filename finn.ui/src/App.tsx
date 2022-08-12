import React from "react";
import { Button, Col, Row, Space, Steps, Upload, UploadFile } from "antd";
import "./App.css";
import { apiFetch } from "./service";
import {
  CloudDownloadOutlined,
  HighlightOutlined,
  SecurityScanOutlined,
  UploadOutlined,
} from "@ant-design/icons";
import { RcFile } from "antd/lib/upload";

function saveAs(url: string, name: string) {
  var a = document.createElement("a");
  a.href = url;
  a.click();
}

function App() {
  const [current, setCurrent] = React.useState(0);
  const [fileList, setFileList] = React.useState<UploadFile[]>([]);
  const [uploading, setUploading] = React.useState(false);

  const handleUpload = async () => {
    setUploading(true);
    setCurrent(0);

    const formData = new FormData();
    fileList.forEach((file) => {
      formData.append("file", file as RcFile);
    });
    var response = await apiFetch("/file/upload", {
      method: "POST",
      body: formData,
    });
    const timer = setInterval(async () => {
      response = await apiFetch(response.headers.get("location")!);
      var body = await response.json();
      switch (body.status) {
        case "Reading":
          setCurrent(1);
          break;
        case "Drawing":
          setCurrent(2);
          break;
        case "Ready":
          setCurrent(3);
          setStatus("finish");
          setUploading(false);
          clearInterval(timer);
          saveAs(response.headers.get("location")!, body.output);
          break;
        case "Error":
          setStatus("error");
          setUploading(false);
          clearInterval(timer);
          break;
        default:
          break;
      }
    }, 1000);
  };

  const [status, setStatus] = React.useState<
    "wait" | "process" | "finish" | "error"
  >("wait");

  return (
    <div className="App">
      <Row justify="center" align="top" gutter={32}>
        <Col flex="auto">
          <Steps current={current} status={status}>
            <Steps.Step
              title="上传"
              icon={<UploadOutlined />}
              description={fileList.length === 0 ? "" : fileList[0].name}
            />
            <Steps.Step title="读取" icon={<SecurityScanOutlined />} />
            <Steps.Step title="绘制" icon={<HighlightOutlined />} />
            <Steps.Step title="下载" icon={<CloudDownloadOutlined />} />
          </Steps>
        </Col>
        <Col>
          <Space size="large">
            <Upload
              accept=".xlsx"
              maxCount={1}
              showUploadList={false}
              onRemove={(file) => {
                const index = fileList.indexOf(file);
                const newFileList = fileList.slice();
                newFileList.splice(index, 1);
                setFileList(newFileList);
              }}
              beforeUpload={(file) => {
                setFileList([...fileList, file]);
                setCurrent(0);
                setStatus("wait");
                return false;
              }}
            >
              <Button icon={<UploadOutlined />}>Select File</Button>
            </Upload>
            <Button
              type="primary"
              onClick={handleUpload}
              disabled={fileList.length === 0}
              loading={uploading}
            >
              {uploading ? "Uploading" : "Start Upload"}
            </Button>
          </Space>
        </Col>
      </Row>
    </div>
  );
}

export default App;
