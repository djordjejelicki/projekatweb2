import React, {useState} from "react";
import axios from "axios";
import Item from "../Models/Item";

const ItemContext = React.createContext({
    onFetch: ()=>{},
});

export const ItemContextProvider = (props) => {
    const [items,setItems] = useState([]);

    const fetchHandler=(items)=>{
        axios.get(process.env.REACT_APP_SERVER_URL+'items/allItems')
        .then(response => {
        if(response.data != null){
            setItems(response.data.map(element => new Item(element)));
        }
        else
        setItems([])
        });
    }

    return (
        <ItemContext.Provider
        value={{
            items:items,
            onFetch: fetchHandler, 
            }}>
            {props.children}
        </ItemContext.Provider>
    )

}

export default ItemContext;
