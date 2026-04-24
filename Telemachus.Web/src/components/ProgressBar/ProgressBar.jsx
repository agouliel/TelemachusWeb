import * as S from './ProgressBar.styled'

const ProgressBar = ({ value = 100, noGutters = false }) => {
  return (
    <div className={`${noGutters ? '' : ' py-4'}`}>
      <div className="progress">
        <S.Container
          className="progress-bar progress-bar-striped progress-bar-animated"
          role="progressbar"
          aria-valuenow={value}
          aria-valuemin="0"
          aria-valuemax="100"
          value={value}
        />
      </div>
    </div>
  )
}

export default ProgressBar
