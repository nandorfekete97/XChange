import { useEffect, useState } from 'react';
import './CurrencyConverter.css';

function CurrencyConverter() {
  const [currencies, setCurrencies] = useState([]);
  const [amountInEuro, setAmountInEuro] = useState('');
  const [selectedCurrencyRate, setSelectedCurrencyRate] = useState(null);
  const [convertedAmount, setConvertedAmount] = useState('');

  const getAllCurrencies = async () => {
    const response = await fetch(`/api/Currency/GetAllCurrencies`);
    if (response.ok) {
      const data = await response.json();
      setCurrencies(data.currencyModels);
      if (data.currencyModels.length > 0) {
        fetchRateAndConvert(data.currencyModels[0].id);
      }
    } else {
      const data = await response.json();
      throw new Error(data);
    }
  };

  const fetchRateAndConvert = async (currencyId) => {
    const response = await fetch(
      `/api/Currency/GetLastCurrencyRatesByCurrencyIds?currencyIds=${currencyId}`
    );

    if (response.ok) {
      const data = await response.json();
      const rate = data[0];
      setSelectedCurrencyRate(rate);

      if (!isNaN(amountInEuro) && amountInEuro !== '') {
        const result = parseFloat(amountInEuro) * rate.rate;
        setConvertedAmount(result.toFixed(2));
      } else {
        setConvertedAmount('');
      }
    } else {
      const data = await response.json();
      throw new Error(data);
    }
  };

  useEffect(() => {
    getAllCurrencies();
  }, []);

  const handleAmountChange = (e) => {
    const value = e.target.value;
    setAmountInEuro(value);

    if (selectedCurrencyRate && !isNaN(value) && value !== '') {
      const result = parseFloat(value) * selectedCurrencyRate.rate;
      setConvertedAmount(result.toFixed(2));
    } else {
      setConvertedAmount('');
    }
  };

  const handleCurrencyChange = (e) => {
    fetchRateAndConvert(e.target.value);
  };

  return (
    <div className="converter-container">
      <h2>Convert from EUR</h2>
      <form className="currency-form">
        <div className="form-item">
          <label htmlFor="eur-input">EUR</label>
          <input
            id="eur-input"
            type="text"
            value={amountInEuro}
            onChange={handleAmountChange}
            placeholder="Enter EUR amount"
          />
        </div>

        <div className="form-item">
          <label htmlFor="currency-select">Currency</label>
          <select
            id="currency-select"
            value={selectedCurrencyRate?.currencyModel?.id || ''}
            onChange={handleCurrencyChange}
          >
            {currencies.map((currency) => (
              <option key={currency.id} value={currency.id}>
                {currency.name} ({currency.shortName})
              </option>
            ))}
          </select>
        </div>

        <div className="form-item">
          <label htmlFor="converted-amount">Converted</label>
          <input
            id="converted-amount"
            type="text"
            value={convertedAmount}
            readOnly
          />
        </div>

        <div className="form-item">
          <button type="submit">Submit</button>
        </div>
      </form>
    </div>
  );
}

export default CurrencyConverter;
