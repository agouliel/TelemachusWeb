import styled from 'styled-components'

export const Container = styled.div`
  width: 100%;
  height: 150px;
  /* border-top: 1px solid #e5e5e5; */
  /* border-bottom: 1px solid #e5e5e5; */
  padding-left: 27px;
  display: flex;
  flex-direction: column;
  justify-content: space-evenly;
  cursor: pointer;
  /* background-color: ${({ isActive }) => (isActive ? '#2ab7f5' : 0)}; */
  /* color: ${({ isActive }) => (isActive ? 'white' : 'black')}; */
  /* position: relative; */
`

export const AddButton = styled.button`
  width: 50px;
  height: 30px;
  position: absolute;
  right: 10px;
  color: black;
`

export const DateContainer = styled.div`
  width: 100%;
  display: flex;
`

export const OptionWrapper = styled.div`
  display: grid;
  grid-template-columns: 1fr 2fr;
`

export const Label = styled.span`
  font-style: normal;
  font-weight: 700;
  font-size: 14px;
  /* line-height: 16px; */
`

export const Value = styled.span`
  font-style: normal;
  margin-left: 30px;
  font-weight: 500;
  font-size: 14px;
  /* line-height: 16px; */
`
