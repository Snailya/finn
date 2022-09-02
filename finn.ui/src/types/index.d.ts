declare module "three-dxf-loader";

declare module "dxf-viewer" {
  export class DxfViewer {
    constructor(container: HTMLDivElement, options: any = null);
    HasRender();
    GetCanvas();
    SetSize(width, height);
    async Load({ url, fonts = null, progressCbk = null, workerFactory = null });
    Render();
    GetLayers();
    ShowLayer(name, show);
    Clear();
    Destroy();
    SetView(center, width);
    /** Set view to fit the specified bounds. */
    FitView(minX, maxX, minY, maxY, padding = 0.1);

    /** @return {Scene} three.js scene for the viewer. Can be used to add custom entities on the
     *      scene. Remember to apply scene origin available via GetOrigin() method.
     */
    GetScene();

    /** @return {Camera} three.js camera for the viewer. */
    GetCamera();

    /** @return {Vector2} Scene origin in global drawing coordinates. */
    GetOrigin();
    /** Subscribe to the specified event. The following events are defined:
     *  * "loaded" - new scene loaded.
     *  * "cleared" - current scene cleared.
     *  * "destroyed" - viewer instance destroyed.
     *  * "resized" - viewport size changed. Details: {width, height}
     *  * "pointerdown" - Details: {domEvent, position:{x,y}}, position is in scene coordinates.
     *  * "pointerup"
     *  * "viewChanged"
     *  * "message" - Some message from the viewer. {message: string, level: string}.
     *
     * @param eventName {string}
     * @param eventHandler {function} Accepts event object.
     */
    Subscribe(eventName, eventHandler);
    /** Unsubscribe from previously subscribed event. The arguments should match previous
     * Subscribe() call.
     *
     * @param eventName {string}
     * @param eventHandler {function}
     */
    Unsubscribe(eventName, eventHandler);
  }
}
