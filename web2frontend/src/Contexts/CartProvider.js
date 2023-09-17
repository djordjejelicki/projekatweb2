import React, {useReducer, useContext} from "react";
import CartContext from "./cart-context";
import ItemContext from "./item-context";

const defaultCartState = {
    items: [],
    totalAmount: 0,
  };
  
const cartReducer = (state, action) => {
    if (action.type === "ADD") {
      if(action.item.amount > action.item.remainingAmount){
        return {
          items: state.items,
          totalAmount: state.totalAmount ,
        };
      }
      const updatedTotalAmount = state.totalAmount + action.item.price * action.item.amount;
      
      const existingIndex = state.items.findIndex(
        (item) => item.id === action.item.id
      );
  
      const existingCartItem = state.items[existingIndex];
      let updatedItems;
      
      if(existingCartItem){
        if(existingCartItem.amount + action.item.amount <= action.item.remainingAmount){
  
        const updatedItem =
        {
          ...existingCartItem, 
          amount: existingCartItem.amount+action.item.amount
        };
        
        updatedItems = [...state.items];
        updatedItems[existingIndex] = updatedItem;
        }
        else
        {
          return {
            items: state.items,
            totalAmount: state.totalAmount ,
          };
        }
      }
      else
      {
        updatedItems= state.items.concat(action.item);
      }
  
      return {
        items: updatedItems,
        totalAmount: updatedTotalAmount,
      };
    }
  
    if(action.type === "REMOVE"){
      
      const existingIndex = state.items.findIndex(
        (item) => item.id === action.id
        );
        
      const existingCartItem = state.items[existingIndex];
      const updatedTotalAmount = state.totalAmount - existingCartItem.price;
      let updatedItems;
      if(existingCartItem.amount === 1){
        updatedItems = state.items.filter(item => item.id !== action.id);
      }else{
        const updatedItem = { ...existingCartItem , amount: existingCartItem.amount-1};
        updatedItems=[...state.items];
        updatedItems[existingIndex]=updatedItem;
      }
  
      return {
        items: updatedItems,
        totalAmount: updatedTotalAmount
      }
    }
  
    if(action.type === "REMOVEALL"){
      return {
        items: [],
        totalAmount: 0
      }
    }
    return defaultCartState;
  };
  
  const CartProvider = (props) => {
    const ctx = useContext(ItemContext)
    const [cartState, dispatchCartAction] = useReducer(
      cartReducer,
      defaultCartState
    );
  
    const addItemToCartHandler = (item) => {
      console.log(item);
      const i =ctx.items.filter(i => i.Id === item.id)
      dispatchCartAction({ type: "ADD", item: {...item, remainingAmount:i[0].Amount}});
    };
  
    const removeItemToCarthandler = (id) => {
      dispatchCartAction({ type: "REMOVE", id: id });
    };
  
    const emptyCartHandler=()=>{
      dispatchCartAction({type: "REMOVEALL"})
    }
  
    const cartContext = {
      items: cartState.items,
      totalAmount: cartState.totalAmount,
      addItem: addItemToCartHandler,
      removeItem: removeItemToCarthandler,
      emptyCart: emptyCartHandler
    };
  
    return (
     
      <CartContext.Provider value={cartContext}>
        {props.children}
      </CartContext.Provider>
      
    );
  };
  
  export default CartProvider;
  