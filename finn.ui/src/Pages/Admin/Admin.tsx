import {
  Button,
  Divider,
  Empty,
  List,
  message,
  Skeleton,
  Typography,
} from "antd";
import { useCallback, useEffect, useRef, useState } from "react";
import { apiFetch } from "../../service";
import "./Admin.css";
import VirtualList from "rc-virtual-list";
import Upload from "antd/lib/upload/Upload";
import { UploadOutlined } from "@ant-design/icons";
import { UploadChangeParam, UploadFile } from "antd/lib/upload";

interface BlockDefinition {
  id: number;
  name: string;
  filename: string;
}

const ContainerHeight = 500;

const Admin: React.FC = () => {
  const [data, setData] = useState<BlockDefinition[]>([]);
  const [previewUrl, setPreviewUrl] = useState<string>();
  const pageNumber = useRef(1);

  const onPreviewClick = useCallback((id: number) => {
    setPreviewUrl(`${process.env.REACT_APP_BACKEND_URL}/blocks/${id}`);
  }, []);

  const onDeleteClick = useCallback((block: BlockDefinition) => {
    apiFetch(`/blocks/${block.id}`, { method: "DELETE" })
      .then((res) => res.json())
      .then((body) => {
        if (body.code === 0) {
          message.success(`${block.name}删除成功。`);

          const index = data.indexOf(block);
          if (index !== -1) {
            data.splice(index, 1);
            setData(data);
          }
        } else {
          message.error(`${block.name}删除失败。`);
        }
      });
  }, []);

  const appendData = () => {
    apiFetch(`/blocks?PageNumber=${pageNumber.current}&PageSize=10`)
      .then((res) => res.json())
      .then((body) => {
        if (body.code === 0) {
          message.success(`${body.data.length} more items loaded!`);

          setData(data.concat(body.data));
          ++pageNumber.current;
        }
      });
  };

  useEffect(() => {
    pageNumber.current = 1;
    appendData();
  }, []);

  const onScroll = (e: React.UIEvent<HTMLElement, UIEvent>) => {
    if (
      e.currentTarget.scrollHeight - e.currentTarget.scrollTop ===
      ContainerHeight
    ) {
      appendData();
    }
  };

  const onUploadChange = (info: UploadChangeParam<UploadFile>) => {
    if (info.file.status === "done") {
      console.log(info.file);
      if (info.file.response.code === 0)
        message.success(`${info.file.name}上传成功`);
      else
        message.error(`${info.file.name}上传失败: ${info.file.response.msg}`);
    } else if (info.file.status === "error") {
      message.error(`${info.file.name}上传失败`);
    }
  };

  return (
    <div className="admin">
      <List
        className="admin__list"
        split
        header={
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
        }
      >
        <VirtualList
          data={data}
          height={ContainerHeight}
          itemHeight={48}
          itemKey="id"
          onScroll={onScroll}
        >
          {(item: BlockDefinition) => (
            <List.Item
              style={{ overflow: "clip" }}
              key={item.id}
              actions={[
                <Button type="link" onClick={() => onPreviewClick(item.id)}>
                  预览
                </Button>,
                <Button type="link" danger onClick={() => onDeleteClick(item)}>
                  删除
                </Button>,
              ]}
            >
              <>{item.name}</>
            </List.Item>
          )}
        </VirtualList>
      </List>
      <div className="admin__viewer">
      </div>
    </div>
  );
};

export default Admin;
