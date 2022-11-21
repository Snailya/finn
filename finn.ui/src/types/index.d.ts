declare module "three-dxf-loader";

declare module "dxf-viewer" {
    export class DxfViewer {
        /** @param domContainer Container element to create the canvas in. Usually empty div. Should not
         *  have padding if auto-resize feature is used.
         * @param options Some options can be overridden if specified. See DxfViewer.DefaultOptions.
         */
        constructor(domContainer, options: any = null);

        /** @return {boolean} True if renderer exists. May be false in case when WebGL context is lost
         * (e.g. after wake up from sleep). In such case page should be reloaded.
         */
        HasRenderer();

        GetCanvas() {
            return this.canvas;
        }

        SetSize(width, height);

        /** Load DXF into the viewer. Old content is discarded, state is reset.
         * @param url {string} DXF file URL.
         * @param fonts {?string[]} List of font URLs. Files should have typeface.js format. Fonts are
         *  used in the specified order, each one is checked until necessary glyph is found. Text is not
         *  rendered if fonts are not specified.
         * @param progressCbk {?Function} (phase, processedSize, totalSize)
         *  Possible phase values:
         *  * "font"
         *  * "fetch"
         *  * "parse"
         *  * "prepare"
         * @param workerFactory {?Function} Factory for worker creation. The worker script should
         *  invoke DxfViewer.SetupWorker() function.
         */
        async Load({url, fonts = null, progressCbk = null, workerFactory = null});

        Render();

        /** @return {Iterable<{name:String, color:number}>} List of layer names. */
        GetLayers();

        ShowLayer(name, show);

        Clear();

        Destroy();

        /** Reset the viewer state. */
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

declare module "dto" {
    export interface RequestLogDto {
        id: number;
        status: string;
        origin: string
        input: string;
        output: string;
        type: string;
        created: moment.Moment;
    }

    export interface CostDto {
        category: string;
        value: number;
    }

    export interface FormulaDto {
        id: number;
        type: string;
        expression: string;
    }

    export interface BlockDefinitionDto {
        id: number;
        name: string;
        filename: string;
    }

    export type TableRecord<T> = T & { key: string | number };
}
