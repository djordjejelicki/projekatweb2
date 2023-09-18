import React, { useContext, useEffect } from "react";
import classes from "./NewOrders.module.css";
import OrderContext from "../../Contexts/order-context";
import AuthContext from "../../Contexts/auth-context";
import { useNavigate } from "react-router-dom";
import OrderCard from "../OrderCard";
import axios from "axios";
import {v4 as uuidv4} from 'uuid';
import { Fragment } from "react";
import Card from "../../components/UI/Card/Card";

const NewOrders = () => {
    const ctx = useContext(OrderContext);
    const authCtx = useContext(AuthContext);
    const navigate = useNavigate();

    useEffect(() => {
        ctx.onFetchNew();
    }, []);

    const SendHandler = (event) => {
      try{
        const response = axios.post(process.env.REACT_APP_SERVER_URL+'orders/sendItem?id='+event.target.id,{
          headers: {
            Authorization: `Bearer ${authCtx.user.Token}`
          }
        });
  
        if(response.data)
          navigate('/')
      }
      catch (error){
        alert(error);
      }
    
    }

    return(
    <Fragment>
    <section className={classes.summary}>
      <h2>New orders</h2>

      <section className={classes.users}>
        {ctx.newOrders.length > 0 ? (
          <Card>
            <ul>
              

              {ctx.newOrders.map((order) => (
                <>
                <OrderCard
                    key={uuidv4()}
                    id={order.id}
                    Items={order.orderItems}
                    shipped={order.shipped}
                    canceled={order.canceled}
                    minutes={order.minutes}
                />
                <button id={order.id}>Send</button>
                </>
              ))}
             
            </ul>
          </Card>
        ) : (
          <h2>You have no new orders</h2>
        )}
      </section>
    </section>
  </Fragment>
    );
};

export default NewOrders;