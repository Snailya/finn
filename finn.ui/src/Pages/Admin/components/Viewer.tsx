import "./Viewer.css";

import { DxfViewer } from "dxf-viewer";
import { useRef } from "react";
import { useEffect } from "react";

export interface ViewerProps {
  url: string | undefined;
}

export const Viewer = (props: ViewerProps) => {
  const viewer = useRef<DxfViewer | null>(null);

  useEffect(() => {
    if (props.url) {
      var dom = document.getElementById("viewer__canvas") as HTMLDivElement;
      if (dom.children.length === 0) {
        viewer.current = new DxfViewer(dom, {
          canvasWidth: dom.offsetWidth,
          canvasHeight: dom.offsetHeight,
          autoResize: true,
        });
      }

      viewer.current?.Clear();
      viewer.current?.Load({ url: props.url });
      return () => viewer.current?.Clear();
    }
  }, [props.url]);

  return (
    <>
      <div id="viewer__canvas"></div>
    </>
  );
};
