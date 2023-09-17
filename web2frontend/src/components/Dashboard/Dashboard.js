import React, { Fragment, useContext, useEffect } from "react";
import classes from './Dashboard.module.css';
import ItemContext from "../../Contexts/item-context";
import Card from "../UI/Card/Card";
import DashboardItem from "./DashboardItem";

const Dashboard = () => {
   const ctx = useContext(ItemContext);
   useEffect(() => {
    ctx.onFetch();
   },[]);

    return(
        <Fragment>
            <section className={classes.summary}>
                <h2>items availbale at store</h2>
                {ctx.items.length > 0 ? (
                    <Card>
                        <section className={classes.items}>
                            <ul>{ctx.items.map((item) => <DashboardItem key={item.Id} id={item.Id} name={item.Name} description={item.Description} price={item.Price} amount={item.Amount} picture={item.PictureUrl}/>)}</ul>
                        </section>
                    </Card>
                ) : (

                    <h2>no availbale items at the moment</h2>
                )}
            </section>
        </Fragment>
    );
};

export default Dashboard;