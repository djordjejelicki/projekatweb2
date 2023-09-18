import OrderItem from './OrderItem'

class Order {
    constructor(id,comment,address,city,zip) {
      this.UserId=id;
      this.Items = [];
      this.Comment=comment;
      this.Address=address;
      this.City=city;
      this.Zip=zip;
    }
    
    addOrderItem(id, amount) {
      this.Items=[...this.Items, new OrderItem(id,amount)];
    }
  }
  
  export default Order;