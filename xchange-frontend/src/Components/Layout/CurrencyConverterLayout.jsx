import React from 'react';
import CurrencyConverter from '../CurrencyConverter/CurrencyConverter';
import './CurrencyConverterLayout.css';

const CurrencyConverterLayout = () => {
  return (
    <div className="layout-container">
      <div className="topbar-row">
        <header className="topbar"> Convert </header>
        <header className="topbar"> Funds </header>
        <header className="topbar"> Exchange History </header>
      </div>
      <main className="main-content">
        <CurrencyConverter/>
      </main>
    </div>
  );
};

export default CurrencyConverterLayout;
