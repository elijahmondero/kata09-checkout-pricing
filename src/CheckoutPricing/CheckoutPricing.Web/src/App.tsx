import React, { useState } from 'react';
import { AppBar, Toolbar, Typography, Container, Grid, Paper, Button } from '@mui/material';
import ProductList from './ProductList';
import Cart from './Cart';
import CheckoutForm from './CheckoutForm';
import TotalAmount from './TotalAmount';
import Footer from './Footer';

function App() {
  const [sessionStarted, setSessionStarted] = useState(false);

  const handleStartSession = () => {
    setSessionStarted(true);
  };

  return (
    <div>
      <AppBar position="static">
        <Toolbar>
          <Typography variant="h6">
            Self Checkout Counter
          </Typography>
        </Toolbar>
      </AppBar>
      <Container>
        {!sessionStarted ? (
          <Button variant="contained" color="primary" onClick={handleStartSession}>
            Start
          </Button>
        ) : (
          <Grid container spacing={3}>
            <Grid item xs={12} md={8}>
              <Paper>
                <ProductList />
              </Paper>
            </Grid>
            <Grid item xs={12} md={4}>
              <Paper>
                <Cart />
                <TotalAmount />
                <CheckoutForm />
              </Paper>
            </Grid>
          </Grid>
        )}
      </Container>
      <Footer />
    </div>
  );
}

export default App;
