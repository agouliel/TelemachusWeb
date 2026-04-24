import styled from 'styled-components'

export const Header = styled.header`
  height: 150px;
  /* background-color: #efefef; */
  display: flex;
  flex-direction: row;
  align-items: center;
  justify-content: space-between;
  padding: 0 60px;
`

export const LogoContainer = styled.div`
  display: flex;
  align-items: center;
  cursor: pointer;
`

export const UserContainer = styled.div`
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-self: center;
`

export const UserName = styled.span`
  font-size: 14px;
  color: black;
  margin: 12px 0 15px 0;
`

export const LogoutButton = styled.button`
  height: 43px;
  width: 117px;
  background: #556080;
  border-radius: 4px;
  color: white;
`

export const Logo = styled.img`
  width: 68px;
  height: 68px;
`

export const Title = styled.h3`
  font-weight: 700;
`
