import React, { useState } from 'react';
import { List, ListItem, ListItemText, Button, TextField } from '@mui/material';

const products = [
  { id: 1, name: 'Product 1', price: 10 },
  { id: 2, name: 'Product 2', price: 20 },
  // Add more products as needed
];

function ProductList() {
  const [searchTerm, setSearchTerm] = useState('');

  const handleSearchChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setSearchTerm(event.target.value);
  };

  const filteredProducts = products.filter(product =>
    product.name.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <div>
      <TextField
        label="Search Products"
        variant="outlined"
        fullWidth
        margin="normal"
        value={searchTerm}
        onChange={handleSearchChange}
      />
      <List>
        {filteredProducts.map(product => (
          <ListItem key={product.id}>
            <ListItemText primary={product.name} secondary={`$${product.price}`} />
            <Button variant="contained" color="primary">Add to Cart</Button>
          </ListItem>
        ))}
      </List>
    </div>
  );
}

export default ProductList;
