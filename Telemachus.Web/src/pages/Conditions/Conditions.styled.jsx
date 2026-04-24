import styled from 'styled-components'

export const Container = styled.div`
  flex: 1;
  /* background-color: #efefef; */
  display: flex;
  flex-direction: column;
  .pagination {
    align-self: flex-start;
    padding-right: 16px;
    margin-left: 10px;
  }
  .Dropdown-root {
    min-width: 150px;
  }
`

export const Header = styled.header`
  height: 200px;
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

export const Label = styled.div`
  margin: 10px 0;
`

export const LogoutButton = styled.button`
  height: 43px;
  width: 117px;
  background: #556080;
  border-radius: 4px;
  color: white;
`

export const ApplyFilterButton = styled.button`
  /* height: 43px;
  width: 117px; */
  background: #556080;
  border-radius: 4px;
  /* margin-left: 20px; */
  color: white;
  &:disabled {
    background: #666;
  }
`

export const Logo = styled.img`
  width: 68px;
  height: 68px;
`

export const Title = styled.span`
  font-size: 40px;
  font-weight: 700;
  color: black;
  margin-left: 11px;
`

export const IconButton = styled.span`
  &:hover {
    opacity: 0.5;
    cursor: pointer;
  }
`

export const ButtonsContainer = styled.div`
  /* display: flex;
  flex-direction: row;
  padding-left: 20px; */
`

export const DropdownsContainer = styled.div`
  display: flex;
  gap: 0 1rem;
  flex-direction: row;
  justify-content: space-around;
  & > {
    display: flex;
    flex-grow: 1;
  }
`

export const Devider = styled.div`
  height: 30px;
  width: 2px;
  background-color: #e0dcdc;
  margin: 0 10px;
`

export const OptionsList = styled.div`
  width: 100%;
  height: 100vh;
  background-color: white;
  overflow-y: scroll;
`

export const Icon = styled.img`
  width: 30px;
  height: 30px;
`

export const LoaderContainer = styled.div`
  width: 100%;
  height: 100px;
  display: flex;
  justify-content: center;
  align-items: center;
`

export const UpdateLoaderContainer = styled.div`
  /* display: flex;
  justify-content: center;
  align-items: center; */
  position: absolute;
  top: 50%;
  left: 50%;

  transform: translate(-50%, -50%);
`
