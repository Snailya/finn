import React, {useEffect, useRef, useState} from "react";
import axios from "axios";
import {Button, Card, Checkbox, Col, Divider, Image, Row, Space, Typography} from "antd";
import {CheckboxChangeEvent} from "antd/es/checkbox";
import "./Slide.css";

interface Thumbnail {
    id: number;
    image: string;
    selected: boolean;
}

export const Slide = () => {
    const [thumbnails, setThumbnails] = useState<Thumbnail[]>([]);

    useEffect(() => {
        axios.create().get("http://localhost:5000/thumbnails").then(res => {
            setThumbnails(res.data.map((item: any) => {
                return {
                    ...item, checked: false
                }
            }));
        });
    }, [])

    const handleSelectedChange = (changed: Thumbnail) => {
        setThumbnails(x => {
            const index = x.findIndex((element) => element.id === changed.id);
            const previous = [...x];
            previous.splice(index, 1, changed)
            return previous;
        })

    }

    const selected = thumbnails.filter(x => x.selected).map(x => x.id);
    const toggleSelectAll = (e: CheckboxChangeEvent) => {
        if (e.target.checked)
            setThumbnails(x => {
                return x.map(item => {
                    return {...item, selected: true}
                })
            })
        else
            setThumbnails(x => {
                return x.map(item => {
                    return {...item, selected: false}
                })
            })
    };
    return (
        <>
            <Row style={{marginBottom: "16px"}}>
                <Col flex={"auto"} style={{display: "flex"}}>
                    <Space>
                        <Checkbox onChange={toggleSelectAll}>全选</Checkbox>
                        {
                            selected.length > 0 && <Typography>已选: {selected.join(", ")}</Typography>
                        }
                    </Space>
                </Col>
                <Col>
                    <Button type={"primary"}
                            href={`http://localhost:5000/merge?${selected.map(id => `pages=${id}`).join("&")}`}>生成</Button>
                </Col>
            </Row>
            <Card>
                {thumbnails.map(thumbnail => (
                    <Card.Grid style={{position: "relative", padding: "0", width: "20%"}} key={thumbnail.id}>
                        <SelectableThumbnail data={thumbnail}
                                             onChange={handleSelectedChange}/>
                    </Card.Grid>
                ))}
            </Card>
        </>

    )
}

const SelectableThumbnail = ({
                                 data,
                                 onChange,
                             }: { data: Thumbnail, onChange: (self: Thumbnail) => void }) => {
    const handleChange = (event: CheckboxChangeEvent) => {
        onChange({...data, selected: event.target.checked});
    }
    return (
        <>
            <Image key={data.id}
                   src={data.image}/>
            <div style={{
                position: "absolute",
                top: 0,
                left: 0,
                width: "100%",
                height: "100%",
                display: data.selected ? "block" : "none",
                background: "rgb(0 0 0 / 50%)",
            }}/>
            <Checkbox style={{position: "absolute", top: 0, left: 0}} checked={data.selected}
                      onChange={handleChange}/>
        </>

    )
}