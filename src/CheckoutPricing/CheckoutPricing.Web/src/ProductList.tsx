import React, { useState, useEffect } from 'react';
import { List, ListItem, ListItemText, Button, TextField } from '@mui/material';
import axios from 'axios';

// Define the Product interface
interface Product {
  id: number;
  name: string;
  unitPrice: number;
}

function ProductList() {
  const [products, setProducts] = useState<Product[]>([]);
  const [searchTerm, setSearchTerm] = useState('');

  useEffect(() => {
    const fetchProducts = async () => {
      try {
        const response = await axios.get<Product[]>('http://localhost:5062/Product');
        setProducts(response.data);
      } catch (error) {
        console.error('Error fetching products:', error);
      }
    };

    fetchProducts();
  }, []);

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
            <ListItemText primary={product.name} secondary={`$${product.unitPrice}`} />
            <Button variant="contained" color="primary">Add to Cart</Button>
          </ListItem>

        ))}
      </List>
    </div>
  );
}

export default ProductList;
