import {TableRecord} from "dto";

export interface TableProps<T> {
    data: TableRecord<T>[];
}
