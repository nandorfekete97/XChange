import { useState } from 'react'
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import './App.css'
import CurrencyConverterLayout from './Components/CurrencyConverterLayout';

function App() {

  return (
    <>
      <Router>
        <Routes>
          <Route path="/" element={ <CurrencyConverterLayout />} />        
        </Routes>
      </Router>
    </>
  )
}

export default App
