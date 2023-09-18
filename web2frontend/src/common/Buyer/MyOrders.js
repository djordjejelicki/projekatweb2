import React, {Fragment, useContext, useEffect } from "react"
import {v4 as uuidv4} from "uuid";
import Card from "../../components/UI/Card/Card";
import OrderCard from "../OrderCard";
import classes from "./MyOrders.module.css";
import OrderContext from "../../Contexts/order-context";


const MyOrders = () => {
    const ctx = useContext(OrderContext);

    useEffect(() => {
        ctx.onFetchBuyers();
    },[]);

    return (
        <Fragment>
          <section className={classes.summary}>
            <h2>My orders</h2>
    
            <section className={classes.users}>
              {ctx.buyersOrders.length > 0 ? (
                <Card>
                  <ul>
                    {ctx.buyersOrders.map((order) => (
                      <OrderCard
                        key={uuidv4()}
                        id={order.id}
                        Items={order.orderItems}
                        shipped={order.shipped}
                        canceled={order.canceled}
                        minutes={order.minutes}
                      />
                    ))}
                  </ul>
                </Card>
              ) : (
                <h2>You never ordered anything</h2>
              )}
            </section>
          </section>
        </Fragment>
      );
}

export default MyOrders;