import React, { useContext,useState } from "react";
import axios from "axios";
import classes from "./Cart.module.css";
import Modal from "../UI/Modal/Modal";
import Input from "../UI/Input/Input";
import CartContext from "../../Contexts/cart-context";
import AuthContext from "../../Contexts/auth-context";
import CartItem from "./CartItem";

const Cart = props => {
    const ctx = useContext(CartContext);
    const authCtx = useContext(AuthContext);
    
    const [checkedOut, setCheckedOut] = useState(false);
    const [addressIsValid, SetAddressIsValid] = useState(false);
    const [cityIsValid, SetCityIsValid] = useState(false);
    const [zipsIsValid, SetZipIsValid] = useState(false);
    
    const totalAmount = `$${ctx.totalAmount.toFixed(2)}`;
    const hasItems = ctx.items.length > 0;

    const cartItemRemoveHandler = (id) => {
        ctx.removeItem(id)
    };
    
    const cartItemAddHandler = (item) => {
        ctx.addItem({ ...item, amount: 1 });
    };

    const cartItems = (
        <ul className={classes["cart-items"]}>
        {ctx.items.map((item) => (
        <CartItem
          key={item.id}
          name={item.name}
          amount={item.amount}
          price={item.price}
          onRemove={cartItemRemoveHandler.bind(null, item.id)}
          onAdd={cartItemAddHandler.bind(null, item)}
        />
      ))}
    </ul>
    );
    
    const OrderHandler = async() => {

    };

    const checkoutHandler = () => {
        setCheckedOut(!checkedOut);
    }
    
    const addressChange = () => {
        if (document.getElementById('address').value === "")
          SetAddressIsValid(false);
        else
          SetAddressIsValid(true);
    }

    const cityChange = () => {
        if (document.getElementById('city').value === "")
          SetCityIsValid(false);
        else
          SetCityIsValid(true);
    }
      
    const zipChange = () => {
        if (document.getElementById('zip').value === "")
          SetZipIsValid(false);
        else
          SetZipIsValid(true);
    }

    const checkout = (
        <div>
          <b><label>Comment:</label></b><br />
          <textarea id='comment' name="comment"></textarea>
          <label style={{color:'red'}}>Fileds with * must not be empty</label>
          <Input id='address' label='Address: *' isValid={addressIsValid} type='text' name='address' onBlur={addressChange}></Input>
          <Input id='city' label='City: *' type="text" name="city" isValid={cityIsValid} onBlur={cityChange}></Input>
          <Input id='zip' label='Zip/Postal code: *' type="text" isValid={zipsIsValid} onBlur={zipChange}></Input>
          <center><button className={classes["button--alt"]} onClick={checkoutHandler}>
            Close
          </button>
    
            {hasItems && <button onClick={OrderHandler} className={classes.button}>Order</button>}
          </center>
        </div>
    );
    
    return(
        <Modal onClose={props.onClose}>
        {checkedOut ? checkout :
        (<>{cartItems}
          <div className={classes.total}>
            <span>Total Amount</span>
            <span>{totalAmount}</span>
          </div>
          <div className={classes.actions}>
            <button className={classes["button--alt"]} onClick={props.onClose}>
              Close
            </button>
            {hasItems && <button onClick={checkoutHandler} className={classes.button}>Checkout</button>}
          </div></>)}
        </Modal>
    );
};

export default Cart;