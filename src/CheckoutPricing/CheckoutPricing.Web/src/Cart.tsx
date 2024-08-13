import React from 'react';
import { List, ListItem, ListItemText, Button } from '@mui/material';

function Cart() {
  // This should be replaced with actual cart state
  const cartItems = [
    { id: 1, name: 'Product 1', price: 10, quantity: 1 },
    // Add more cart items as needed
  ];

  return (
    <List>
      {cartItems.map(item => (
        <ListItem key={item.id}>
          <ListItemText primary={item.name} secondary={`$${item.price} x ${item.quantity}`} />
          <Button variant="contained" color="secondary">Remove</Button>
        </ListItem>
      ))}
    </List>
  );
}

export default Cart;
