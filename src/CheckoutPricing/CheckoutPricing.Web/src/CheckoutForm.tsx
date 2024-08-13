import React from 'react';
import { TextField, Button } from '@mui/material';

function CheckoutForm() {
  return (
    <form>
      <TextField label="Card Number" fullWidth margin="normal" />
      <TextField label="Expiry Date" fullWidth margin="normal" />
      <TextField label="CVV" fullWidth margin="normal" />
      <Button variant="contained" color="primary" fullWidth>Checkout</Button>
    </form>
  );
}

export default CheckoutForm;
