import {TableViewModel} from "dto";

export interface TableProps<T> {
    data: TableViewModel<T>[];
}
