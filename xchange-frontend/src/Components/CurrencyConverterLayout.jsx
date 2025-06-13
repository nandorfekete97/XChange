import React from 'react'
import CurrencyConverter from './CurrencyConverter';
import { Layout } from 'antd';
import Profile from './Profile';
const { Header, Footer, Sider, Content } = Layout;

const CurrencyConverterLayout = () => {
  return (
      <Layout width="100%">
        <Sider style={{ width:'450', minWidth:'450px'}}>
          <Profile/>
        </Sider>
        <Content>
          <CurrencyConverter/>
        </Content>
      </Layout>
  )
}

export default CurrencyConverterLayout
