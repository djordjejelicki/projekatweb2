import React,{useContext} from "react";
import classes from "./OrderCard.module.css";
import AuthContext from "../Contexts/auth-context";

const OrderCard = (props) => {
    const ctx= useContext(AuthContext)
    return (
      <li className={classes.user}>
        <div>
          <h4>Order Id: {props.id}</h4>
          {props.Buyer && (<h3>Buyer: {props.Buyer}</h3>)}
          <div className={classes.description}>
            {props.Items.map(element => <p>Item name:<b>{element.item.name}</b> Ordered amount: <b>{element.amount}</b> Price: <b>{element.item.price}$</b></p>)}
          </div>
          <h4>Time until arrival {props.minutes}min</h4>
          {(ctx.user.Role===1 && !props.shipped) ? (
            <button >Cancel</button>
          ):
          (<></>)}
          
        </div>
      </li>
    )
  }
  
  export default OrderCard