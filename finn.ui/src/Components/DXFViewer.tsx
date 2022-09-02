import { DxfViewer } from "dxf-viewer";
import React, { useEffect, useRef } from "react";

export interface DXFViewerProps {
  url: string | undefined;
}

function DXFViewer(props: DXFViewerProps) {
  const canvas = useRef(null);

  useEffect(() => {
    if (canvas.current) {
      const dxfViewer = new DxfViewer(canvas.current, null);
      if (props.url !== "")
        dxfViewer.Load({
          url: props.url,
        });
    }
  }, [props.url]);

  return <div ref={canvas}></div>;
}

export default DXFViewer;
