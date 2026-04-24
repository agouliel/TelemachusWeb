import styled from 'styled-components'

export const Container = styled.div`
  background-color: #efefef;
  min-height: 100vh;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  font-size: calc(10px + 2vmin);
  color: white;
`

export const Form = styled.form`
  width: 100%;
  max-width: 330px;
  padding: 15px;
  margin: 0 auto;
  & input[type!='password'] {
  }
  & input[type='password'] {
  }
`

export const FormContainer = styled.div`
  display: -ms-flexbox;
  display: -webkit-box;
  display: flex;
  -ms-flex-align: center;
  -ms-flex-pack: center;
  -webkit-box-align: center;
  align-items: center;
  -webkit-box-pack: center;
  justify-content: center;
  padding-top: 40px;
  padding-bottom: 40px;
  /* background-color: #f5f5f5; */
  text-align: center;
  height: 100vh;
  overflow-y: hidden;
`
