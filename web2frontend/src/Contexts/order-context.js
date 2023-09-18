import React, { useContext, useState } from "react";
import axios from "axios";
import AuthContext from "./auth-context";

const OrderContext = React.createContext({
    onFetchBuyers: ()=>{},
    onFetchNew: ()=>{},
    onFetchAll: ()=>{},
    onFetchHistory: ()=>{}
});

export const OrderContextProvider = props => {
    const ctx = useContext(AuthContext);
    const [buyerOrders, setBuyersOrders] = useState([]);
    const [newOrders, setNewOrders] = useState([]);
    const [allOrders, setAllOrders] = useState([]);
    const [orderHistory, setOrderHistory] = useState([]);

    const fetchBuyersHandler=()=>{
        axios.get(process.env.REACT_APP_SERVER_URL+'orders/myOrders?id='+ctx.user.Id,{
            headers: {
              Authorization: `Bearer ${ctx.user.Token}`
            }
          })
        .then(response => {
            if(response.data != null){
                setBuyersOrders(response.data);
            }
            else
            setBuyersOrders([])
    });
    }

    const fetchNewHandler = () => {
        axios.get(process.env.REACT_APP_SERVER_URL+'orders/newOrders?id='+ctx.user.Id, {
            headers: {
              Authorization: `Bearer ${ctx.user.Token}`
            }
          })
        .then(response => {
        if(response.data != null){
          setNewOrders(response.data);
        }
        else
        setNewOrders([])
        });
    }

    const fetchAllHandler=()=>{
        axios.get(process.env.REACT_APP_SERVER_URL+'orders/allOrders',{
            headers: {
              Authorization: `Bearer ${ctx.user.Token}`
            }
          })
        .then(response => {
        if(response.data != null){
            setAllOrders(response.data);
        }
        else
        setAllOrders([])
        });
    }

    const fetchHistoryHandler = () =>{
        axios.get(process.env.REACT_APP_SERVER_URL+'orders/orderHistory?id='+ctx.user.Id,{
            headers: {
              Authorization: `Bearer ${ctx.user.Token}`
            }
          })
        .then(response => {
        if(response.data != null){
            setOrderHistory(response.data);
        }
        else
        setOrderHistory([])
        });
    }

    return(
        <OrderContext.Provider
            value={{
                buyersOrders: buyerOrders,
                newOrders: newOrders,
                allOrders: allOrders,
                orderHistory: orderHistory,
                onFetchHistory: fetchHistoryHandler,
                onFetchBuyers: fetchBuyersHandler,
                onFetchNew: fetchNewHandler,
                onFetchAll: fetchAllHandler
            }}>
            {props.children}
        </OrderContext.Provider>
    );
}

export default OrderContext;